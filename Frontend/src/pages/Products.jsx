/**
 * Page‑level wrapper that simply renders our reusable <Products /> list.
 * If you don’t need a dedicated page component, you can delete this file
 * and route directly to /components/product‑list/Products.jsx.
 */
import Products from "@/components/product-list/Products";

export default function ProductsPage() {
  return <Products />;
}
