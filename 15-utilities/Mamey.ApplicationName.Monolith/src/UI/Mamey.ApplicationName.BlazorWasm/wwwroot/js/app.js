window.App = {

    // Save data to sessionStorage
    setSessionStorage: function (key, value) {
        sessionStorage.setItem(key, value);
    },

    // Get data from sessionStorage
    getSessionStorage: function (key) {
        return sessionStorage.getItem(key);
    },

    // Remove data from sessionStorage
    removeSessionStorage: function (key) {
        sessionStorage.removeItem(key);
    },

    // Dynamically load a JavaScript file
    loadScript: function (url, callback) {
        const script = document.createElement('script');
        script.src = url;
        script.onload = callback;
        script.onerror = function () {
            console.error(`Failed to load script: ${url}`);
        };
        document.head.appendChild(script);
    },

    // Set the document title
    setDocumentTitle: function (title) {
        document.title = title;
    },

    focusElement: (element) => {
        if (element) {
            element.focus();
        }
    },
    togglePopover: function (popoverId) {
        const popover = document.getElementById(popoverId);
        if (popover) {
            if (isOpen) {
                popover.classList.add('open');
            } else {
                popover.classList.remove('open');
            }
        }
    },
    getCookie: function (name) {
        const nameEQ = name + "=";
        const ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i].trim();
            if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    setCookie: function (name, value) {
        console.log(`Setting session cookie: ${name}`);
        document.cookie = `${name}=${value || ""}; path=/; Secure; SameSite=None`;
    },
    deleteCookie: function (name) {
        console.log(`Deleting cookie: ${name}`);
        document.cookie = `${name}=; Max-Age=-99999999; path=/; Secure; SameSite=None`;
    },
};
