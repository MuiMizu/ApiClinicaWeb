import { reactive } from 'vue';

export const authState = reactive({
    isLoggedIn: !!localStorage.getItem('token')
});

export const login = (token) => {
    localStorage.setItem('token', token);
    authState.isLoggedIn = true;
};

export const logout = () => {
    localStorage.removeItem('token');
    authState.isLoggedIn = false;
};
