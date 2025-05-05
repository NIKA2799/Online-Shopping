import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import Product from "@/components/product/Product";
const fetchProducts = async () => {
  const { data } = await axios.get("https://dummyjson.com/products");
  const products = Array.isArray(data) ? data : data.products ?? [];

  console.log("[API] fetched products", products); // ← one log per fetch
  return products;
};

export default function Products() {
  /* ---------------------------------------------------------------
   * React Query v5 — object signature
   * --------------------------------------------------------------- */
  const {
    data: products = [],
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ["products", "all"],
    queryFn: fetchProducts,
    staleTime: 5 * 60 * 1000, // 5‑minute freshness
    cacheTime: 30 * 60 * 1000, // 30‑minute cache
    keepPreviousData: true,
    refetchOnWindowFocus: false,
  });

  /* ---------------------------------------------------------------
   * Render states
   * --------------------------------------------------------------- */
  if (isLoading) {
    return <section className="p-4">Loading products…</section>;
  }

  if (isError) {
    return (
      <section className="p-4 text-red-600">
        Error loading products: {error.message}
      </section>
    );
  }

  /* ---------------------------------------------------------------
   * Product grid
   * --------------------------------------------------------------- */
  return (
    <section className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-4 p-4">
      {products.length === 0 ? (
        <p>No products found.</p>
      ) : (
        products
          .filter((p) => p?.id ?? p?._id) // skip items without an id
          .map((p) => <Product key={p.id ?? p._id ?? p.name} product={p} />)
      )}
    </section>
  );
}
