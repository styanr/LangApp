const { getDefaultConfig } = require('expo/metro-config');
const { withNativeWind } = require('nativewind/metro');

const config = getDefaultConfig(__dirname);

// axios issue
config.resolver.resolveRequest = function packageExportsResolver(context, moduleImport, platform) {
  // Use the browser version of the package for React Native
  if (moduleImport === 'axios' || moduleImport.startsWith('axios/')) {
    return context.resolveRequest(
      {
        ...context,
        unstable_conditionNames: ['browser'],
      },
      moduleImport,
      platform
    );
  }

  // Fall back to normal resolution
  return context.resolveRequest(context, moduleImport, platform);
};

module.exports = withNativeWind(config, { input: './global.css' });
