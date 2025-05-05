import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";

export default function ProductDetails() {
  const { productId } = useParams();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [mainImg, setMainImg] = useState("");

  /* ------------------------------------------------------------------
   * Fetch the product once the id is available
   * ------------------------------------------------------------------ */
  useEffect(() => {
    async function loadProduct() {
      try {
        setLoading(true);
        setError("");

        // Replace with your real API / query‑client as needed
        const res = await fetch(`https://dummyjson.com/products/${productId}`);
        if (!res.ok) throw new Error("Product not found");

        const data = await res.json();
        setProduct(data);
        setMainImg(data.thumbnail || data.images?.[0] || "");
      } catch (err) {
        setError(err.message || "Unexpected error");
      } finally {
        setLoading(false);
      }
    }

    loadProduct();
  }, [productId]);

  /* ------------------------------------------------------------------
   * Helpers
   * ------------------------------------------------------------------ */
  const discountedPrice =
    product && product.discountPercentage
      ? (product.price * (1 - product.discountPercentage / 100)).toFixed(2)
      : product?.price;

  /* ------------------------------------------------------------------
   * Render
   * ------------------------------------------------------------------ */
  if (loading) {
    return (
      <div className="container mx-auto max-w-4xl p-6">
        <p className="animate-pulse text-lg text-gray-500">Loading…</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto max-w-4xl p-6">
        <p className="text-lg font-semibold text-red-600">{error}</p>
      </div>
    );
  }

  return (
    <div className="container mx-auto max-w-4xl p-6">
      {/* Breadcrumb‑style heading */}
      <h1 className="mb-6 text-3xl font-extrabold">{product.title}</h1>

      <div className="grid gap-10 md:grid-cols-2">
        {/* LEFT: main image & thumbnails */}
        <div>
          {mainImg && (
            <img
              src={mainImg}
              alt={product.title}
              className="mb-4 h-80 w-full rounded-2xl object-contain shadow"
            />
          )}

          {/* Thumbnails */}
          <div className="flex gap-3 overflow-x-auto">
            {product.images?.map((img) => (
              <button
                key={img}
                className={`h-20 w-20 flex-shrink-0 overflow-hidden rounded-lg border-2 ${
                  img === mainImg ? "border-indigo-500" : "border-transparent"
                }`}
                onClick={() => setMainImg(img)}
              >
                <img
                  src={img}
                  alt="Product thumbnail"
                  className="h-full w-full object-cover"
                />
              </button>
            ))}
          </div>
        </div>

        {/* RIGHT: details */}
        <div className="space-y-4">
          {/* Price block */}
          <div className="flex items-end gap-3">
            <span className="text-3xl font-bold">${discountedPrice}</span>
            {product.discountPercentage && (
              <span className="text-sm text-gray-500 line-through">
                ${product.price.toFixed(2)}
              </span>
            )}
            {product.discountPercentage && (
              <span className="rounded bg-rose-100 px-2 py-0.5 text-xs font-semibold text-rose-600">
                −{product.discountPercentage}%
              </span>
            )}
          </div>

          {/* Availability */}
          <p
            className={`font-medium ${
              product.stock > 0
                ? "text-emerald-600"
                : "text-gray-500 line-through"
            }`}
          >
            {product.availabilityStatus}
          </p>

          {/* Rating */}
          <p className="text-sm text-gray-600">
            Rating: {product.rating?.toFixed(2)} / 5
          </p>

          {/* Brand & SKU */}
          <p className="text-sm text-gray-600">
            Brand: <span className="font-medium">{product.brand}</span>
          </p>
          <p className="text-sm text-gray-600">SKU: {product.sku}</p>

          {/* Description */}
          <p className="text-gray-700">{product.description}</p>

          {/* Meta ‑ quick list */}
          <ul className="space-y-1 text-sm text-gray-700">
            <li>
              <span className="font-medium">Category:</span> {product.category}
            </li>
            <li>
              <span className="font-medium">Shipping:</span>{" "}
              {product.shippingInformation}
            </li>
            <li>
              <span className="font-medium">Return policy:</span>{" "}
              {product.returnPolicy}
            </li>
            <li>
              <span className="font-medium">Warranty:</span>{" "}
              {product.warrantyInformation}
            </li>
          </ul>

          {/* Tags */}
          {product.tags?.length > 0 && (
            <div className="flex flex-wrap gap-2 pt-2">
              {product.tags.map((tag) => (
                <span
                  key={tag}
                  className="rounded-full bg-indigo-100 px-3 py-1 text-xs font-semibold text-indigo-600"
                >
                  #{tag}
                </span>
              ))}
            </div>
          )}

          {/* Action buttons */}
          <button
            disabled={product.stock === 0}
            className={`mt-4 w-full rounded-xl px-6 py-3 text-center text-sm font-semibold text-white shadow transition
              ${
                product.stock === 0
                  ? "cursor-not-allowed bg-gray-400"
                  : "bg-indigo-600 hover:bg-indigo-700"
              }`}
            onClick={() => {
              /* TODO: integrate with cart */
            }}
          >
            {product.stock === 0 ? "Out of Stock" : "Add to Cart"}
          </button>
        </div>
      </div>
    </div>
  );
}
