using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Linq;
using System;
using Org.BouncyCastle.Math.EC;

namespace ADHNetworkShared.Crypto {
    public static class ECIES {

        public static AsymmetricCipherKeyPair GenerateECIESKeyPair() {

            var keyGen = new ECKeyPairGenerator();
            var keyGenParams = new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256k1, new SecureRandom());
            keyGen.Init(keyGenParams);

            return keyGen.GenerateKeyPair();

        }

        public static byte[] EncryptECIES(byte[] data, ECPublicKeyParameters publicKey, AsymmetricCipherKeyPair tempPair) {

            // 임시 키 페어 생성
            var ephemeralKeyPair = tempPair;
            var ephemeralPublicKey = (ECPublicKeyParameters)ephemeralKeyPair.Public;
            var ephemeralPrivateKey = (ECPrivateKeyParameters)ephemeralKeyPair.Private;

            // ECDH로 공유 비밀 생성
            var agreement = new ECDHBasicAgreement();
            agreement.Init(ephemeralPrivateKey);
            var sharedSecret = agreement.CalculateAgreement(publicKey);

            // KDF로 암호화 키 유도
            var kdf = new Sha256Digest();
            var encryptionKey = new byte[32]; // AES-256 키 크기
            var sharedSecretBytes = sharedSecret.ToByteArrayUnsigned();
            kdf.BlockUpdate(sharedSecretBytes, 0, sharedSecretBytes.Length);
            kdf.DoFinal(encryptionKey, 0);

            // AES 암호화
            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
            var keyParam = new KeyParameter(encryptionKey);
            var iv = new byte[16];
            new SecureRandom().NextBytes(iv);
            var parameters = new ParametersWithIV(keyParam, iv);
            cipher.Init(true, parameters);

            var encryptedData = new byte[cipher.GetOutputSize(data.Length)];
            var len = cipher.ProcessBytes(data, 0, data.Length, encryptedData, 0);
            cipher.DoFinal(encryptedData, len);

            // 결과 조합: 임시 공개키 + IV + 암호문
            using (var ms = new MemoryStream()) {
                // 임시 공개키 좌표 저장
                var encodedPoint = ephemeralPublicKey.Q.GetEncoded();
                ms.Write(BitConverter.GetBytes(encodedPoint.Length), 0, 4);
                ms.Write(encodedPoint, 0, encodedPoint.Length);

                // IV 저장
                ms.Write(iv, 0, iv.Length);

                // 암호문 저장
                ms.Write(encryptedData, 0, encryptedData.Length);

                return ms.ToArray();
            }

        }

        public static byte[] DecryptECIES(byte[] cipherData, ECPrivateKeyParameters privateKey, out ECPublicKeyParameters senderPubKey) {
            return DecryptECIESInternal(cipherData, privateKey, out senderPubKey);
        }

        public static byte[] DecryptECIES(byte[] cipherData, ECPrivateKeyParameters privateKey) {
            return DecryptECIESInternal(cipherData, privateKey, out _);
        }

        public static byte[] DecryptECIESInternal(byte[] cipherData, ECPrivateKeyParameters privateKey, out ECPublicKeyParameters senderPubKey) {

            using (var ms = new MemoryStream(cipherData))
            using (var reader = new BinaryReader(ms)) {
                // 임시 공개키 복원
                var pointLength = reader.ReadInt32();
                var encodedPoint = reader.ReadBytes(pointLength);
                var curve = ECNamedCurveTable.GetByName("secp256k1");
                var ephemeralPublicKey = new ECPublicKeyParameters(
                    curve.Curve.DecodePoint(encodedPoint),
                    new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H));

                senderPubKey = ephemeralPublicKey;

                // IV 읽기
                var iv = reader.ReadBytes(16);

                // 암호문 읽기
                var remainingData = new byte[ms.Length - ms.Position];
                ms.Read(remainingData, 0, remainingData.Length);

                // ECDH로 공유 비밀 생성
                var agreement = new ECDHBasicAgreement();
                agreement.Init(privateKey);
                var sharedSecret = agreement.CalculateAgreement(ephemeralPublicKey);

                // KDF로 복호화 키 유도
                var kdf = new Sha256Digest();
                var decryptionKey = new byte[32];
                var sharedSecretBytes = sharedSecret.ToByteArrayUnsigned();
                kdf.BlockUpdate(sharedSecretBytes, 0, sharedSecretBytes.Length);
                kdf.DoFinal(decryptionKey, 0);

                // AES 복호화
                var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
                var keyParam = new KeyParameter(decryptionKey);
                var parameters = new ParametersWithIV(keyParam, iv);
                cipher.Init(false, parameters);

                var decryptedData = new byte[cipher.GetOutputSize(remainingData.Length)];
                var len = cipher.ProcessBytes(remainingData, 0, remainingData.Length, decryptedData, 0);
                var finalLength = cipher.DoFinal(decryptedData, len);

                // 패딩을 제거한 실제 데이터 길이만큼 반환
                return decryptedData.Take(len + finalLength).ToArray();
            }

        }


        public static ECPublicKeyParameters RestorePublicBytesToKey(byte[] bytes) {

            var curve = SecNamedCurves.GetByName("secp256k1");
            ECPoint point = curve.Curve.DecodePoint(bytes);

            return new ECPublicKeyParameters(point, new ECDomainParameters(curve));

        }

        public static ECPrivateKeyParameters RestorePrivateBytesToKey(byte[] bytes) {

            var curve = SecNamedCurves.GetByName("secp256k1");
            Org.BouncyCastle.Math.BigInteger big = new Org.BouncyCastle.Math.BigInteger(bytes);

            return new ECPrivateKeyParameters(big, new ECDomainParameters(curve));

        }

    }


}
