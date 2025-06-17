using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using MemoryPack;
using MemoryPack.Compression;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Buffers;

namespace StressClient {

    public class Client {

        public static HttpClient HttpClient { get; set; }
        public byte[] AESKey { get; set; }
        public byte[] ServerCommonKey { get; set; }
        public long UID { get; set; }
        public string AuthToken { get; set; }
        private string ServerURL { get; set; }
        private string Version { get; set; }

        public string PW { get; set; }

        static Client() {
            HttpClient = new HttpClient();
        }

        public Client(string ServerURL, string Version, string pw) { 

            this.ServerURL = ServerURL;
            this.Version = Version;
            this.PW = pw;
        }

        private async Task<ProtocolRes> GetRequestAsync(ProtocolReq req) {

            using (HttpResponseMessage m = await HttpClient.GetAsync($"{ServerURL}{Router.RoutingMap[req.protocolID]}"))
            {
                EncryptedData data = MemoryPackSerializer.Deserialize<EncryptedData>(await m.Content.ReadAsByteArrayAsync());
                byte[] plain = AES.DecryptAES(data.Data, AESKey, data.IV);

                ProtocolRes res = null;

                if (m.Headers.TryGetValues("IsCompress", out var isCompress)) {

                    if (isCompress.First() == "true") {

                        using var decompressor = new BrotliDecompressor();
                        res = MemoryPackSerializer.Deserialize<ProtocolRes>(decompressor.Decompress(plain).ToArray());

                    } else res = MemoryPackSerializer.Deserialize<ProtocolRes>(plain);

                } else res = MemoryPackSerializer.Deserialize<ProtocolRes>(plain);
                
                return res;

            }


        }
        private async Task<ProtocolRes> PostRequestAsync(ProtocolReq req) {

            if (AESKey == null) {
                Console.WriteLine("error: require login auth!!");
                return null;
            }

            (byte[] encryptedReq, byte[] iv) = AES.EncryptAES(MemoryPackSerializer.Serialize(req), AESKey);

            using (HttpResponseMessage m = await HttpClient.PostAsync($"{ServerURL}{Router.RoutingMap[req.protocolID]}", new ByteArrayContent(MemoryPackSerializer.Serialize(new ClientEncryptedData(encryptedReq, iv, UID))))) {
                
                EncryptedData data = MemoryPackSerializer.Deserialize<EncryptedData>(await m.Content.ReadAsByteArrayAsync());
                byte[] plain = AES.DecryptAES(data.Data, AESKey, data.IV);
                ProtocolRes res = null;
                
                if (m.Headers.TryGetValues("IsCompress", out var isCompress)) {

                    if (isCompress.First() == "true") {
                        
                        using var decompressor = new BrotliDecompressor();
                        res = MemoryPackSerializer.Deserialize<ProtocolRes>(decompressor.Decompress(plain).ToArray());

                    } else res = MemoryPackSerializer.Deserialize<ProtocolRes>(plain);

                } else res = MemoryPackSerializer.Deserialize<ProtocolRes>(plain);
               
                //Debug.Log(m.ToString());

                return res;
            }

        }
        private async Task<ProtocolRes> PostNoneAuthRequestAsync(ProtocolReq req) {

            var pair = ECIES.GenerateECIESKeyPair();

            byte[] data = ECIES.EncryptECIES(MemoryPackSerializer.Serialize(req), ECIES.RestorePublicBytesToKey(ServerCommonKey), pair);

            using (HttpResponseMessage m = await HttpClient.PostAsync($"{ServerURL}{Router.RoutingMap[req.protocolID]}", new ByteArrayContent(data))) {

                byte[] plain = ECIES.DecryptECIES(await m.Content.ReadAsByteArrayAsync(), pair.Private as ECPrivateKeyParameters);
                ProtocolRes res = null;

                if (m.Headers.TryGetValues("IsCompress", out var isCompress)) {

                    if (isCompress.First() == "true") {

                        using var decompressor = new BrotliDecompressor();
                        res = MemoryPackSerializer.Deserialize<ProtocolRes>(decompressor.Decompress(plain).ToArray());

                    } else res = MemoryPackSerializer.Deserialize<ProtocolRes>(plain);

                } else res = MemoryPackSerializer.Deserialize<ProtocolRes>(plain);

                return res;
            }

        }
        private bool TryParseResponse<T>(ProtocolRes res, out T targetType) where T : ProtocolRes {

            bool flag = res != null && res is T && res.Result == ErrorCode.None;

            if (flag) {
                targetType = res as T;
                //Debug.Log(res.ToString());
            } else {
                targetType = null;
                Console.WriteLine(res.ToString());
            }

            return flag;

        }
        private async Task<DtoHandShakeRes> HandShakeInner() {

            if (ServerCommonKey != null) return null;

            ProtocolReq req = new DtoHandShakeReq() { Version = Version };

            using (HttpResponseMessage res = await HttpClient.PostAsync($"{ServerURL}{Router.RoutingMap[req.protocolID]}", new ByteArrayContent(MemoryPackSerializer.Serialize(req)))) {

                byte[] buffer = await res.Content.ReadAsByteArrayAsync();
                DtoHandShakeRes handRes = MemoryPackSerializer.Deserialize<ProtocolRes>(buffer) as DtoHandShakeRes;

                if (handRes.VersionOK) ServerCommonKey = handRes.ServerCommonKey;
                else {
                    Console.WriteLine(res.ToString());
                    Console.WriteLine(handRes.ToString());
                    return null;
                }

                Console.WriteLine("handshake: Success get temp key!");
                return handRes;

            }

        }
        private async Task<DtoHandShakeRes> HandShake(int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                return await HandShakeInner()
                            .TimeoutAfter(TimeSpan.FromSeconds(15));

            } catch (OperationCanceledException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await HandShake(curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());
            }

            return null;

        }
        private async Task<DtoLoginRes> LoginRequest(string id, string pw, int curRetryCnt = 0, int maxRetryCnt = 3) {

            var clientKeyPair = ECDH.GenerateECKeyPair();
            var clientPrivateKey = clientKeyPair.Private as ECPrivateKeyParameters;
            var clientPublicKey = clientKeyPair.Public as ECPublicKeyParameters;

            try {

                var req = new DtoLoginReq(id, pw, clientPublicKey.Q.GetEncoded());

                var baseRes = await PostNoneAuthRequestAsync(req)
                                   .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<DtoLoginRes>(baseRes, out var res)) {

                    UID = res.Uid;
                    AuthToken = res.AuthToken;
                    AESKey = ECDH.GenerateSharedSecret(clientPrivateKey, ECDH.RestorePublicBytesToKey(res.ServerPublicKey));

                    return res;

                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await LoginRequest(id, pw, curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());
            }


            return null;
        }
        private async Task<BasicProtocolRes> CreateAccountRequest(string id, string pw, int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var req = new DtoAccountRegisterReq(id, pw);

                var baseRes = await PostNoneAuthRequestAsync(req)
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                    return res;
                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await CreateAccountRequest(id, pw, curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());
            }

            return null;

        }
        private async Task<PostTestRes> AuthPostTest(string msg, int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var req = new AuthPostTestReq(msg, AuthToken, UID);

                var baseRes = await PostRequestAsync(req)
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<PostTestRes>(baseRes, out var res)) {
                    return res;
                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await AuthPostTest(msg, curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

            }


            return null;

        }
        private async Task<BasicProtocolRes> LogoutRequest(int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var req = new DtoLogoutReq(AuthToken, UID);

                var baseRes = await PostRequestAsync(req)
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                    
                    Reset();

                    return res;
                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await LogoutRequest(curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());
            }

            return null;

        }
        private async Task<BasicProtocolRes> DeleteAccountRequest(string pw, int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var req = new DtoAccountDeleteReq(AuthToken, UID, pw);

                var baseRes = await PostRequestAsync(req)
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                    Reset();

                    return res;
                }

            } catch (TimeoutException ex) {


                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await DeleteAccountRequest(pw, curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

            }

            return null;

        }
        private async Task<PostTestRes> PostTest(string msg, int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var baseRes = await PostNoneAuthRequestAsync(new PostTestReq(msg))
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<PostTestRes>(baseRes, out var res)) {
                    return res;
                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await PostTest(msg, curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());
            }

            return null;

        }
        private async Task<ItemDBTestRes> ItemDBTest(bool isCache, int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var req = new ItemDBTestReq(AuthToken, UID, isCache);

                var baseRes = await PostRequestAsync(req)
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<ItemDBTestRes>(baseRes, out var res)) {
                    return res;
                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await ItemDBTest(isCache, curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

            }


            return null;

        }
        private async Task<BasicProtocolRes> ItemDBUpdateTest(int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var req = new ItemDBUpdateTestReq(AuthToken, UID);

                var baseRes = await PostRequestAsync(req)
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                    return res;
                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await ItemDBUpdateTest(curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

            }


            return null;

        }
        private async Task<DtoRewardRes> GetGatchaList(int curRetryCnt = 0, int maxRetryCnt = 3) {

            try {

                var req = new DtoGatchaewardReq(AuthToken, UID);

                var baseRes = await PostRequestAsync(req)
                                    .TimeoutAfter(TimeSpan.FromSeconds(15));

                if (TryParseResponse<DtoRewardRes>(baseRes, out var res)) {
                    return res;
                }

            } catch (TimeoutException ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

                if (curRetryCnt >= maxRetryCnt) {

                    Console.WriteLine($"request fail - retry cnt over");
                    return null;

                } else {

                    Console.WriteLine($"retry - {curRetryCnt + 1}");
                    return await GetGatchaList(curRetryCnt + 1, maxRetryCnt);

                }

            } catch (Exception ex) {

                Console.WriteLine(ex);
                Console.WriteLine(ex.ToString());

            }


            return null;

        }


        public void Reset() {

            UID = default;
            AuthToken = default;
            AESKey = null;
            ServerCommonKey = null;

        }
        
        public async Task<DtoHandShakeRes> HandShake() => await HandShake(0, 3);
        public async Task<DtoLoginRes> LoginRequest(string id, string pw) => await LoginRequest(id, pw, 0, 3);
        public async Task<BasicProtocolRes> CreateAccountRequest(string id, string pw) => await CreateAccountRequest(id, pw, 0, 3);
        public async Task<PostTestRes> AuthPostTest(string msg) => await AuthPostTest(msg, 0, 3);
        public async Task<PostTestRes> PostTest(string msg) => await PostTest(msg, 0, 3);
        public async Task<BasicProtocolRes> LogoutRequest() => await LogoutRequest(0, 3);
        public async Task<BasicProtocolRes> DeleteAccountRequest() => await DeleteAccountRequest(PW, 0, 3);
        public async Task<ItemDBTestRes> ItemDBTest(bool isCache) => await ItemDBTest(isCache, 0, 3);
        public async Task<BasicProtocolRes> ItemDBUpdateTest() => await ItemDBUpdateTest(0, 3);
        public async Task<DtoRewardRes> GetGatchaList() => await GetGatchaList(0, 3);

    }

}
