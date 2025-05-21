import React, { useEffect, useState } from 'react';
import { Image, View } from 'react-native';
import { LucideProps } from 'lucide-react-native';

type ImageWithFallbackProps = {
  imageUrl?: string | null;
  fallbackIcon: React.ReactElement<LucideProps>;
  width: number;
  height: number;
  imageClassName?: string;
  iconContainerClassName?: string;
};

export const ImageWithFallback = ({
  imageUrl,
  fallbackIcon,
  width,
  height,
  imageClassName,
  iconContainerClassName,
}: ImageWithFallbackProps) => {
  const [error, setError] = useState(false);
  const [errorData, setErrorData] = useState(null);

  useEffect(() => {
    console.error(errorData);
    console.log(imageUrl);
  }, [errorData]);

  if (!imageUrl || error) {
    return (
      <View
        style={{ width, height }}
        className={`items-center justify-center bg-gray-200 dark:bg-gray-700 ${iconContainerClassName}`}>
        {React.cloneElement(fallbackIcon, { size: Math.min(width, height) * 0.6 })}
      </View>
    );
  }

  return (
    <Image
      source={{ uri: imageUrl }}
      style={{ width, height }}
      className={imageClassName}
      onError={(e) => {
        setError(true);
        setErrorData(e.nativeEvent);
      }}
    />
  );
};
