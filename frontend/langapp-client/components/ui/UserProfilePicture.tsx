import React, { useState, useEffect } from 'react';
import { useFileAccess } from '@/hooks/useFileAccess';
import { User } from 'lucide-react-native';
import { ImageWithFallback } from './ImageWithFallback';
import { Text } from './text';

type UserProfilePictureProps = {
  imageUrl?: string | null;
  size?: number;
  imageClassName?: string;
  iconContainerClassName?: string;
};

export const UserProfilePicture = ({
  imageUrl,
  size = 40,
  imageClassName: providedImageClassName = '',
  iconContainerClassName: providedIconContainerClassName = '',
}: UserProfilePictureProps) => {
  const { getReadUrl, isLoading: isFetching, error } = useFileAccess();
  const [accessibleUrl, setAccessibleUrl] = useState<string | null>(null);

  const finalImageClassName = `rounded-full ${providedImageClassName}`.trim();
  const finalIconContainerClassName = `rounded-full ${providedIconContainerClassName}`.trim();

  useEffect(() => {
    if (imageUrl) {
      getReadUrl(imageUrl)
        .then((url) => setAccessibleUrl(url))
        .catch(() => setAccessibleUrl(null));
    } else {
      setAccessibleUrl(null);
    }
  }, [imageUrl]);

  // useEffect(() => {
  //   console.log('Accessible URL:', accessibleUrl);
  // }, [accessibleUrl]);

  return (
    <>
      <ImageWithFallback
        imageUrl={accessibleUrl || imageUrl}
        fallbackIcon={<User className="text-indigo-600" />}
        width={size}
        height={size}
        imageClassName={finalImageClassName}
        iconContainerClassName={finalIconContainerClassName}
      />
      {/* <Text>
      {accessibleUrl}
    </Text> */}
    </>
  );
};
