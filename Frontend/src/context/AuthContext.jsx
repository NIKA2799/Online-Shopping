import { createContext, useContext, useState } from "react";
import { decodeToken } from "@/utils/jwt";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  // grab raw token from localStorage
  const [token, setToken] = useState(() => localStorage.getItem("token"));
  // decode once on init, if present
  const [user, setUser] = useState(() => (token ? decodeToken(token) : null));

  const login = (newToken) => {
    localStorage.setItem("token", newToken);
    setToken(newToken);
    setUser(decodeToken(newToken));
  };

  const logout = () => {
    localStorage.removeItem("token");
    setToken(null);
    setUser(null);
  };

  const isGuest = !user;

  return (
    <AuthContext.Provider value={{ token, user, login, logout, isGuest }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
