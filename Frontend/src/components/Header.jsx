// Frontend/src/components/Header.jsx
import headerBg from "@/assets/header.png";

export default function Header() {
  return (
    <header className="fixed inset-x-0 top-0 z-50 h-14 bg-gray-100 shadow flex items-center px-4">
      {}
      <img
        src={headerBg}
        alt="Header graphic"
        className="w-1/5 h-full object-contain"
      />

      {}
    </header>
  );
}
