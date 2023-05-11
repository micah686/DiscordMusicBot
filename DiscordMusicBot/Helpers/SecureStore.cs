using NeoSmart.SecureStore;
using System;
using System.IO;

namespace Yuna.Helpers
{
    public sealed class SecureStore
    {
        private static readonly string SECURE_DB_STORE = "secure.json";
        private static readonly string SECURE_DB_KEY = "secure.key";

        internal static readonly string _token = "DISCORD_TOKEN";

        public static void CreateSecureStore()
        {
            if (!DoesSecureStoreExist())
            {
                using var sman = SecretsManager.CreateStore();
                sman.GenerateKey();

                sman.ExportKey(SECURE_DB_KEY);
                sman.SaveStore(SECURE_DB_STORE);
            }
        }

        public static bool DoesSecureStoreExist()
        {
            return File.Exists(SECURE_DB_STORE) && File.Exists(SECURE_DB_KEY);
        }


        public static bool Update(string key, string value)
        {
            try
            {
                using (var sman = SecretsManager.LoadStore(SECURE_DB_STORE))
                {
                    sman.LoadKeyFromFile(SECURE_DB_KEY);
                    sman.Set(key, value);
                    sman.SaveStore(SECURE_DB_STORE);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static string Decrypt(string key)
        {
            using (var sman = SecretsManager.LoadStore(SECURE_DB_STORE))
            {
                sman.LoadKeyFromFile(SECURE_DB_KEY);

                if (!sman.TryGetValue(key, out string outValue))
                {
                    return string.Empty;
                }
                return outValue;
            }
        }

    }
}
