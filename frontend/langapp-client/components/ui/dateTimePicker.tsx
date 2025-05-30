import { DateTimePickerAndroid } from '@react-native-community/datetimepicker';
import { Button } from './button';
import { Text } from './text';
import { View } from 'react-native';
import { useTranslation } from 'react-i18next';
import { DateDisplay } from '@/components/ui/DateDisplay';

interface DateTimePickerProps {
  date: Date;
  onChange: (selectedDate: Date | undefined) => void;
  mode: 'date' | 'time';
  className?: string;
}

export const DatePicker = ({
  date,
  onChange: onDateChange,
  mode,
  className,
}: DateTimePickerProps) => {
  const showDateTimepicker = () => {
    const safeDate = date instanceof Date ? date : new Date(date);
    DateTimePickerAndroid.open({
      value: safeDate,
      onChange: (event, selectedDate) => {
        if (selectedDate) {
          console.log('Selected date:', selectedDate);
          DateTimePickerAndroid.open({
            value: selectedDate instanceof Date ? selectedDate : new Date(selectedDate),
            onChange: (event2, selectedTime) => {
              if (selectedTime) {
                onDateChange(selectedTime);
              }
            },
            mode: 'time',
            is24Hour: true,
          });
        }
      },
      mode: 'date',
      is24Hour: true,
      minimumDate: new Date(),
    });
  };

  const { t } = useTranslation();

  return (
    <View className={`flex items-start ${className}`}>
      <Button onPress={showDateTimepicker} className="ml-2">
        <Text className="text-sm">{t('dateTimePicker.pickDatePrompt')}</Text>
      </Button>
      <View className="ml-2 mt-2">
        <DateDisplay dateString={date.toISOString()} relative={false} className={'text-sm'} />
      </View>
    </View>
  );
};
