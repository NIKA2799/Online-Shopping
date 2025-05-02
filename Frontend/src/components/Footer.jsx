import { NavLink } from "react-router-dom";

const linkClass = ({ isActive }) =>
  `flex flex-col items-center w-full py-2 truncate ${
    isActive ? "font-semibold" : ""
  }`;

export default function Footer() {
  return (
    <footer className="fixed inset-x-0 bottom-0 z-50 h-14 bg-gray-100 shadow-inner flex justify-around">
      <NavLink to="/" end className={linkClass}>
        <i className="pi pi-home text-lg" />
        <span className="text-xs">Home</span>
      </NavLink>

      <NavLink to="/products" className={linkClass}>
        <i className="pi pi-shopping-bag text-lg" />
        <span className="text-xs">Products</span>
      </NavLink>

      <NavLink to="/cart" className={linkClass}>
        <i className="pi pi-shopping-cart text-lg" />
        <span className="text-xs">Cart</span>
      </NavLink>

      <NavLink to="/about" className={linkClass}>
        <i className="pi pi-info-circle text-lg" />
        <span className="text-xs">About</span>
      </NavLink>

      <NavLink to="/contact" className={linkClass}>
        <i className="pi pi-envelope text-lg" />
        <span className="text-xs">Contact</span>
      </NavLink>
    </footer>
  );
}
