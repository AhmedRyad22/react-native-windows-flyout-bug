import clsx from 'clsx';
import {Fragment, useState} from 'react';
import {Button, Text, TextInput, TouchableOpacity, View} from 'react-native';
import Icon from 'react-native-vector-icons/Feather';
import {Flyout, useColorScheme} from 'react-native-windows';

export default function Storage() {
  return (
    <View className="p-12 space-y-4">
      <Text className="font-medium text-lg">Stockage S3</Text>
      <View className="h-[1px] bg-black" />

      <View className="flex-row space-x-2">
        <AccessConfiguration />
      </View>
    </View>
  );
}

function AccessConfiguration() {
  const colorScheme = useColorScheme();

  const [CredentialsModal, setCredentialsModal] = useState(false);

  const [accessKey, setAccessKey] = useState('Z2PNBJV0IAI5QGIZ0WJ9');
  const [secreetKey, setSecreetKey] = useState(
    'rC2vYY3vkwwurshOgdIY82NV9xSMDFIFYa7nZ0EG',
  );

  return (
    <Fragment>
      <TouchableOpacity
        onPress={e => setCredentialsModal(true)}
        className={clsx(
          'rounded p-5 w-64',
          colorScheme === 'dark' ? 'bg-neutral-900' : 'bg-neutral-200',
        )}>
        <View className="p-6 items-center justify-center">
          <Icon name="key" size={32} />
        </View>
        <Text className="text-center">Configurez vos accès.</Text>
      </TouchableOpacity>

      <Flyout
        isOpen={CredentialsModal}
        onDismiss={() => {
          setCredentialsModal(false);
        }}>
        <View
          className={clsx(
            'w-[380px] p-6 space-y-4 rounded',
            colorScheme === 'dark' ? 'bg-neutral-900' : 'bg-neutral-200',
          )}>
          <Text>New content</Text>
          <TextInput onChangeText={setAccessKey} placeholder="Access key" />
          <TextInput onChangeText={setSecreetKey} placeholder="Secret key" />
          <View className="mt-4 flex-row space-x-1">
            <View className="flex-1">
              <Button
                onPress={async () => {
                  const r = await Managedrives.testConnection(
                    accessKey,
                    secreetKey,
                  );

                  console.log(r);
                }}
                title="Testez les accès"
              />
            </View>

            <View className="flex-1">
              <Button
                onPress={() => {
                  // setMountModal(false);
                }}
                title="Enregistrez les accès"
              />
            </View>
          </View>
        </View>
      </Flyout>
    </Fragment>
  );
}
