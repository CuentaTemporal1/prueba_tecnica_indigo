import { defineStore } from 'pinia';
import api from '../services/api';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    isLoggedIn: false,
  }),
  actions: {
    async login(loginDto) {
      try {
        const { data } = await api.post('/Auth/login', loginDto);
        this.user = data;
        this.isLoggedIn = true;
        
        return [true, this.user.roles[0]];
      } catch (error) {
        this.user = null;
        this.isLoggedIn = false;
        return [false, null];
      }
    },
    async logout() {
      try {
        await api.post('/Auth/logout');
      } catch {}
      this.user = null;
      this.isLoggedIn = false;
    },
    async fetchUser() {
      try {
        const { data } = await api.get('/Auth/me');
        this.user = data;
        this.isLoggedIn = true;
        return true;
      } catch {
        this.user = null;
        this.isLoggedIn = false;
        return false;
      }
    },
  },
});
