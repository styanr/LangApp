import { DateTimePickerAndroid } from '@react-native-community/datetimepicker';
import { Button } from './button';
import { Text } from './text';
import { View } from 'react-native';

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

  return (
    <View className={`flex items-start ${className}`}>
      <Button onPress={showDateTimepicker} className="ml-2">
        <Text className="text-sm">Select Date & Time</Text>
      </Button>
      <View className="ml-2">
        <Text className="text-sm">
          {date instanceof Date ? date.toLocaleDateString() : date} {date.toLocaleTimeString()}
        </Text>
      </View>
    </View>
  );
};
