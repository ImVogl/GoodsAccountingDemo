import { useState } from 'react';
import { useNavigation } from '@react-navigation/native';
import {
    Text,
    SafeAreaView,
    TouchableOpacity,
    StyleSheet,
    TextInput,
    ImageBackground,
    View,
    Alert
} from 'react-native';

import ApiClient from '../../common/ApiClient';

const image = require('../../../assets/background.png');
const LoginScreen = () => {
    const client = new ApiClient();
    const [user, setUser] = useState('');
    const [password, setPassword] = useState('');
    const navigation = useNavigation();
    const handleAsync = (login: string, password: string): Promise<void> => {
        return client.signin(login, password).then(success => {
            if (success) {
                navigation.navigate("Selling" as never, {} as never);
            }
            else {
                Alert.alert("Не удалось авторизоваться");
            }
        }).catch(() => Alert.alert("Не удалось авторизоваться"))
    }
    
    return(
        <SafeAreaView>
            <ImageBackground source={image} style={styles.ImageBackground} >
                <View style={styles.viewinput}>
                    <Text style={styles.text}>Логин</Text>
                    <TextInput style={styles.input} onChangeText={setUser} value={user} />
                </View>
                <View style={styles.viewinput}>
                    <Text style={styles.text}>Пароль</Text>
                    <TextInput style={styles.input} onChangeText={setPassword} value={password} secureTextEntry={true} />
                </View>
                <View style={styles.viewinput}>
                    <TouchableOpacity style={styles.button} onPress={() => handleAsync(user, password) } >
                        <Text style={styles.text_button}>Войти</Text>
                    </TouchableOpacity>
                </View>
            </ImageBackground>
        </SafeAreaView>
    )
}

const styles = StyleSheet.create({
    ImageBackground:{
        width: '100%',
        height: '100%'
    },
    button:{
        height: 40,
        borderWidth: 2,
        borderColor: 'green',
        backgroundColor: 'green',
        borderRadius: 5
    },
    viewinput:{
        padding: 10,
        marginTop: 12,
        marginRight:24,
        marginLeft:24
    },
    input: {
        fontSize: 24,
        borderWidth: 1,
        padding: 10,
        margin: 5
    },
    button_text:{
        fontSize: 24,
        color: 'yellow',
        margin: 'center'
    },
    text_button:{
        fontSize: 24,
        color: 'yellow',
        textAlign: 'center'
    },
    text:{
        fontSize: 24,
        color: 'yellow',
    }
});

export default LoginScreen;