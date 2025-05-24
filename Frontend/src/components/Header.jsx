import { useState } from "react";
import { Link } from "react-router-dom";
import headerBg from "@/assets/header.png";
import { useAuth } from "@/context/AuthContext";

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false);
  const { user, logout, isGuest } = useAuth();

  const toggleMenu = () => setMenuOpen((prev) => !prev);

  return (
    <header className="fixed inset-x-0 top-0 z-50 h-14 bg-gray-100 shadow flex items-center justify-between px-4">
      {/* Logo or image */}
      <img
        src={headerBg}
        alt="Header graphic"
        className="w-1/5 h-full object-contain"
      />

      {/* Burger icon */}
      <div className="relative">
        <button
          className="text-gray-700 focus:outline-none"
          onClick={toggleMenu}
        >
          <svg
            className="w-6 h-6"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M4 6h16M4 12h16M4 18h16"
            />
          </svg>
        </button>

        {/* Dropdown menu */}
        {menuOpen && (
          <div className="absolute right-0 mt-2 w-40 bg-white border rounded shadow-lg z-50">
            {isGuest ? (
              <>
                <Link
                  to="/login"
                  className="block px-4 py-2 text-gray-700 hover:bg-gray-100"
                  onClick={() => setMenuOpen(false)}
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="block px-4 py-2 text-gray-700 hover:bg-gray-100"
                  onClick={() => setMenuOpen(false)}
                >
                  Register
                </Link>
              </>
            ) : (
              <>
                <span className="block px-4 py-2 text-gray-500">
                  {user.email}
                </span>
                <button
                  onClick={() => {
                    logout();
                    setMenuOpen(false);
                  }}
                  className="block w-full text-left px-4 py-2 text-red-600 hover:bg-red-100"
                >
                  Logout
                </button>
              </>
            )}
          </div>
        )}
      </div>
    </header>
  );
}
