import '../styles/site.css';
import 'alpinejs';
import axios from 'axios';
import { library, dom } from "@fortawesome/fontawesome-svg-core";
import { fas } from '@fortawesome/free-solid-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';
import { fab } from '@fortawesome/free-brands-svg-icons';
library.add(fas, far, fab);
dom.watch();
export function setupNav() {
    return {
        toggleMenu() {
            var headerNav = document.getElementById('headerNav');
            if (headerNav) {
                if (headerNav.classList.contains('hidden')) {
                    headerNav.classList.remove('hidden');
                }
                else {
                    headerNav.classList.add('hidden');
                }
            }
        }
    };
}
export function setupUsers() {
    return {
        users: [],
        async mounted() {
            await this.loadUsers();
        },
        async deleteUser(currentUser) {
            if (confirm(`Really delete ${currentUser.firstName} ${currentUser.lastName}?`)) {
                await axios.delete(`${apiHost}/api/users/${currentUser.id}`);
                await this.loadUsers();
            }
        },
        async loadUsers() {
            try {
                const response = await axios.get(`${apiHost}/api/users`);
                this.users = response.data;
            }
            catch (error) {
                console.log(error);
            }
        }
    };
}
export function createOrUpdateUser() {
    return {
        user: {},
        async create() {
            try {
                await axios.post(`${apiHost}/api/users`, this.user);
                window.location.href = "/users";
            }
            catch (error) {
                console.log(error);
            }
        },
        async update() {
            try {
                await axios.put(`${apiHost}/api/users/${this.user.id}`, this.user);
                window.location.href = "/users";
            }
            catch (error) {
                console.log(error);
            }
        },
        async loadData() {
            const pathnameSplit = window.location.pathname.split('/');
            const id = pathnameSplit[pathnameSplit.length - 1];
            try {
                const response = await axios.get(`${apiHost}/api/users/${id}`);
                this.user = response.data;
            }
            catch (error) {
                console.log(error);
            }
        },
    };
}
