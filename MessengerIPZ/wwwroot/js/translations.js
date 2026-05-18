window.translations = {

    uk: {

        // NAVBAR
        chat: 'Мій Чат',
        login: 'Увійти',
        register: 'Реєстрація',
        logout: 'Вийти',

        // SETTINGS
        settings: 'Налаштування',
        language: 'Мова інтерфейсу',
        font: 'Шрифт',
        save: 'Зберегти',
        fontSize: 'Розмір шрифту',

        // INDEX
        welcome: 'Ласкаво просимо до MessengerIPZ',
        subtitle: 'Простий та швидкий спосіб спілкуватися з друзями та колегами у реальному часі.',
        goChats: 'Перейти до чатів',

        // LOGIN
        loginTitle: 'Авторизація',
        username: "Ім'я користувача",
        password: 'Пароль',
        enterLogin: 'Введіть логін',
        enterPassword: 'Введіть пароль',
        noAccount: 'Ще не зареєстровані?',
        createAccount: 'Створити акаунт',
        loginError: 'Невірний логін або пароль!',

        // REGISTER
        registerTitle: 'Створення акаунту',
        registerPassword: 'Пароль (мінімум 6 символів)',
        confirmPassword: 'Підтвердження паролю',
        createLogin: 'Придумайте логін',
        createPassword: 'Придумайте пароль',
        repeatPassword: 'Повторіть пароль',
        alreadyAccount: 'Вже маєте акаунт?',
        passwordMismatch: 'Паролі не співпадають!',
        successRegister: 'Реєстрація успішна! Перенаправлення...',
        registerError: 'Не вдалося зареєструватись.',

        // CHAT
        rooms: 'Кімнати',
        channel: 'Оберіть канал',
        channelName: 'Назва',
        message: 'Повідомлення...',
        join: 'Вступити',
        open: 'Відкрити',
        enterChannel: 'Вступіть у канал, щоб бачити листування',
        chatPrefix: 'Чат: #'
    },

    en: {

        // NAVBAR
        chat: 'My Chat',
        login: 'Login',
        register: 'Register',
        logout: 'Logout',

        // SETTINGS
        settings: 'Settings',
        language: 'Interface language',
        font: 'Font',
        save: 'Save',
        fontSize: 'Font size',

        // INDEX
        welcome: 'Welcome to MessengerIPZ',
        subtitle: 'A simple and fast way to communicate with friends and colleagues in real time.',
        goChats: 'Go to chats',

        // LOGIN
        loginTitle: 'Authorization',
        username: 'Username',
        password: 'Password',
        enterLogin: 'Enter login',
        enterPassword: 'Enter password',
        noAccount: 'Not registered yet?',
        createAccount: 'Create account',
        loginError: 'Invalid username or password!',

        // REGISTER
        registerTitle: 'Create account',
        registerPassword: 'Password (minimum 6 characters)',
        confirmPassword: 'Confirm password',
        createLogin: 'Create login',
        createPassword: 'Create password',
        repeatPassword: 'Repeat password',
        alreadyAccount: 'Already have an account?',
        passwordMismatch: 'Passwords do not match!',
        successRegister: 'Registration successful! Redirecting...',
        registerError: 'Registration failed.',

        // CHAT
        rooms: 'Rooms',
        channel: 'Select channel',
        channelName: 'Name',
        message: 'Message...',
        join: 'Join',
        open: 'Open',
        enterChannel: 'Join the channel to view messages',
        chatPrefix: 'Chat: #'
    }
};

window.getText = function (key) {

    const lang =
        localStorage.getItem('lang') || 'uk';

    return window.translations[lang][key];
}