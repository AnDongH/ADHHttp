using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Shared.Util {
    public static class ConfigData {

        public static IConfigurationRoot _config;

        public static void Setup(IConfigurationRoot config) {
            _config = config;
        }

    }
}
