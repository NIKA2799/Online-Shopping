import { useEffect, useState } from "react";
import axios from "axios";
import Product from "@/components/product/Product";

export default function Products() {
  const [products, setProducts] = useState([]);
  const [loading,  setLoading]  = useState(true);
  const [error,    setError]    = useState(null);

  useEffect(() => {
    axios
      .get("http://backend")        // change to your real endpoint
      .then((res) => {
        setProducts(res.data);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setError(err);
        setLoading(false);
      });
  }, []);

  if (loading) return <section className="p-4">Loading products…</section>;
  if (error)   return <section className="p-4 text-red-600">Error loading products.</section>;

  return (
    <section className="grid gap-4 p-4 grid-cols-[repeat(auto-fill,minmax(220px,1fr))]">
      {products.map((p) => (
        <Product key={p.id ?? p._id ?? p.name} product={p} />
      ))}
    </section>
  );
}
