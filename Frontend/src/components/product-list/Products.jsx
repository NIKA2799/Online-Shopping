import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";

import CategoriesBar from "@/components/category/CategoriesBar";
import Product from "@/components/product/Product";

/* -------------------------------------------------------------
 * Fetch products: by slug, or all
 * ------------------------------------------------------------- */
const fetchProducts = async ({ queryKey }) => {
  const [, category] = queryKey;

  const url =
    category === "all"
      ? "https://dummyjson.com/products"
      : `https://dummyjson.com/products/category/${category}`;

  const { data } = await axios.get(url);
  return Array.isArray(data) ? data : data.products ?? [];
};

export default function Products() {
  const [selected, setSelected] = useState("all");

  const {
    data: products = [],
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ["products", selected],
    queryFn: fetchProducts,
    keepPreviousData: true,
    staleTime: 5 * 60 * 1000,
    cacheTime: 30 * 60 * 1000,
    refetchOnWindowFocus: false,
  });

  return (
    <>
      <CategoriesBar selected={selected} onSelect={setSelected} />

      {isLoading ? (
        <section className="p-4">Loading products…</section>
      ) : isError ? (
        <section className="p-4 text-red-600">
          Error loading products: {error.message}
        </section>
      ) : (
        <section className="grid grid-cols-[repeat(auto-fill,minmax(220px,1fr))] gap-4 p-4">
          {products.length === 0 ? (
            <p>No products found.</p>
          ) : (
            products
              .filter((p) => p?.id ?? p?._id)
              .map((p) => <Product key={p.id ?? p._id ?? p.name} product={p} />)
          )}
        </section>
      )}
    </>
  );
}
