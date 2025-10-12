


importScripts("https://www.gstatic.com/firebasejs/10.13.1/firebase-app-compat.js");
importScripts("https://www.gstatic.com/firebasejs/10.13.1/firebase-messaging-compat.js");


const firebaseConfig = {
    apiKey: "AIzaSyDiATvrg8ADQfUHWoMbIVZkL1JYa85n_3M",
    authDomain: "movielisting-66cce.firebaseapp.com",
    projectId: "movielisting-66cce",
    storageBucket: "movielisting-66cce.firebasestorage.app",
    messagingSenderId: "838477265766",
    appId: "1:838477265766:web:1bd04a3a554e47a24fa6a7"
};

    const app = firebase.initializeApp(firebaseConfig);
    const messaging = firebase.messaging();

messaging.onBackgroundMessage(function (payload) {
    console.log("Background message received:", payload);
    self.registration.showNotification(payload.notification.title, {
        body: payload.notification.body,
    });
});
