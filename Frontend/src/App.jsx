import { Routes, Route, Outlet } from "react-router-dom";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import Home from "@/pages/Home";
import Products from "@/pages/Products";
import Cart from "@/pages/Cart";
import About from "@/pages/About";
import Contact from "@/pages/Contact";

export default function App() {
  return (
    /* Root flex‑column wrapper that always fills the viewport */
    <div className="flex min-h-screen flex-col">
      <Header />

      {/* Top padding = header height, bottom padding = footer height */}
      <main className="flex-1 overflow-y-auto pt-14 pb-14">
        <Routes>
          <Route element={<Outlet />}>
            <Route path="/" element={<Home />} />
            <Route path="/products" element={<Products />} />
            <Route path="/cart" element={<Cart />} />
            <Route path="/about" element={<About />} />
            <Route path="/contact" element={<Contact />} />
          </Route>
        </Routes>
      </main>

      <Footer />
    </div>
  );
}
