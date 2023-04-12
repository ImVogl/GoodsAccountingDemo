import * as React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import SellScreen from '../screens/sell/SellScreen';
import LoginScreen from '../screens/login/LoginScreen';

const Stack = createNativeStackNavigator();
const MainStack = () => {
    return(
        <NavigationContainer>
            <Stack.Navigator>
                <Stack.Screen name="Login" component={LoginScreen} options={ { title: 'Авторизация' } } />
                <Stack.Screen name="Selling" component={SellScreen} options={ { title: 'Продажа', gestureEnabled: false, headerShown: true, headerLeft: () => <></> } } />
            </Stack.Navigator>
        </NavigationContainer>
    )
}

export default MainStack;