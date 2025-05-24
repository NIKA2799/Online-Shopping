import axios from "axios";

const BASE_URL = "https://yourapi.com/api";

export const registerUser = async (userData) => {
  const response = await axios.post(`${BASE_URL}/auth/register`, userData);
  return response.data;
};

export const loginUser = async (credentials) => {
  const response = await axios.post(`${BASE_URL}/auth/login`, credentials);
  return response.data;
};

export const fetchProfile = async (token) => {
  const response = await axios.get(`${BASE_URL}/auth/profile`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return response.data;
};
