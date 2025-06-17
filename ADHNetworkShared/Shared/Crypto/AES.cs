using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using ADHNetworkShared.Protocol;

namespace ADHNetworkShared.Crypto {
    public static class AES {

        private static readonly SecureRandom Random = new SecureRandom();

        // AES 암호화를 위한 키와 IV 생성
        public static byte[] GenerateRandomKey(int size) {
            byte[] key = new byte[size];
            Random.NextBytes(key);
            return key;
        }

        // AES 암호화
        public static (byte[], byte[]) EncryptAES(byte[] plainText, byte[] key) {
            // AES 암호화 엔진 설정

            byte[] iv = GenerateRandomKey(16);
           
            AesEngine aesEngine = new AesEngine(); // 기본 AES 엔진 (ECB 모드)
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(aesEngine)); // CBC 모드 + 패딩
            KeyParameter keyParam = new KeyParameter(key);
            ParametersWithIV keyWithIvParam = new ParametersWithIV(keyParam, iv);
            cipher.Init(true, keyWithIvParam); // true는 암호화 모드

            return (ProcessCipher(cipher, plainText), iv);
        }

        // AES 복호화
        public static byte[] DecryptAES(byte[] cipherText, byte[] key, byte[] iv) {

            // AES 복호화 엔진 설정
            AesEngine aesEngine = new AesEngine();
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(aesEngine)); // CBC 모드 + 패딩
            KeyParameter keyParam = new KeyParameter(key);
            ParametersWithIV keyWithIvParam = new ParametersWithIV(keyParam, iv);
            cipher.Init(false, keyWithIvParam); // false는 복호화 모드

            return ProcessCipher(cipher, cipherText);
        
        }

        // AES 암호화/복호화 처리를 위한 공통 메서드
        private static byte[] ProcessCipher(IBufferedCipher cipher, byte[] input) {
            byte[] outputBytes = new byte[cipher.GetOutputSize(input.Length)];
            int length = cipher.ProcessBytes(input, 0, input.Length, outputBytes, 0);
            try {
                length += cipher.DoFinal(outputBytes, length);
            } catch (CryptoException e) {
                Console.WriteLine("Error during final block processing: " + e.Message);
                return null;
            }
            byte[] result = new byte[length];
            Array.Copy(outputBytes, 0, result, 0, length);
            return result;
        }

    }
}
