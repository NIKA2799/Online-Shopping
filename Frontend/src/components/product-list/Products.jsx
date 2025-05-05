import { useEffect, useState } from "react";
import axios from "axios";
import Product from "@/components/product/Product";

export default function Products() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const { data } = await axios.get("https://dummyjson.com/products"); // swap to your real endpoint when ready
        // The API returns { products: [...] }. Grab the array safely.
        setProducts(Array.isArray(data) ? data : data.products ?? []);
        console.log(data);
      } catch (err) {
        console.error(err);
        setError(err);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  if (loading) return <section className="p-4">Loading products…</section>;
  if (error)
    return (
      <section className="p-4 text-red-600">Error loading products.</section>
    );

  return (
    <section className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-4 p-4">
      {products.length === 0 ? (
        <p>No products found.</p>
      ) : (
        products
          .filter((p) => p?.id ?? p?._id) // ⬅ only render items with an id
          .map((p) => <Product key={p.id ?? p._id} product={p} />)
      )}
    </section>
  );
}
