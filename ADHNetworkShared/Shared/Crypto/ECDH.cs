using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Math.EC;

namespace ADHNetworkShared.Crypto {
    public static class ECDH {

        public static AsymmetricCipherKeyPair GenerateECKeyPair() {
            var keyGen = new ECKeyPairGenerator();
            var keyGenParams = new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, new SecureRandom());
            keyGen.Init(keyGenParams);

            return keyGen.GenerateKeyPair();
        }

        public static byte[] GenerateSharedSecret(ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey) {
            var agreement = new ECDHBasicAgreement();
            agreement.Init(privateKey);
            var sharedSecret = agreement.CalculateAgreement(publicKey);
            byte[] data = sharedSecret.ToByteArray();
            
            if (data.Length >= 33 && data[0] == 0x00) {
                byte[] result = new byte[32];
                Array.Copy(data, 1, result, 0, 32);
                return result;
            }


            return data;
        }

        public static ECPublicKeyParameters RestorePublicBytesToKey(byte[] bytes) {

            var curve = SecNamedCurves.GetByName("secp256r1");
            ECPoint point = curve.Curve.DecodePoint(bytes);

            return new ECPublicKeyParameters(point, new ECDomainParameters(curve));

        }

    }
}
