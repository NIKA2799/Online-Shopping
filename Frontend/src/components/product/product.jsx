import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import PropTypes from "prop-types";
import defaultProductImage from "@/assets/defaultProduct.png";

export default function Product({ product }) {
  /* -------------------------------------------------------------
   * Normalise API fields → local variables
   * ------------------------------------------------------------- */
  const id = product.id ?? product._id;
  const name = product.name ?? product.title ?? "Unnamed product";
  const price = product.price;
  const description = product.description;

  // Choose the first valid image we can find
  const apiImage =
    product.imageUrl || product.thumbnail || product.images?.[0] || null;

  /* -------------------------------------------------------------
   * Image placeholder logic
   * ------------------------------------------------------------- */
  const [imgSrc, setImgSrc] = useState(defaultProductImage);

  useEffect(() => {
    if (apiImage) setImgSrc(apiImage);
  }, [apiImage]);

  const handleError = () => setImgSrc(defaultProductImage);

  /* -------------------------------------------------------------
   * Guard: don’t render a card without an id (nothing to link to)
   * ------------------------------------------------------------- */
  if (id == null) return null;

  /* -------------------------------------------------------------
   * Render
   * ------------------------------------------------------------- */
  return (
    <article className="rounded-lg border border-gray-200 p-4 text-center shadow-sm">
      {/* product image */}
      <img
        src={imgSrc}
        alt={name}
        onError={handleError}
        className="mb-2 h-40 w-full rounded object-cover"
      />

      {/* name */}
      <h2 className="truncate text-base font-medium">{name}</h2>

      {/* price + details button */}
      <div className="mt-1 flex items-center justify-between">
        {price != null && (
          <p className="text-sm font-semibold text-gray-500">
            ${Number(price).toFixed(2)}
          </p>
        )}

        <Link
          to={`/products/${id}`}
          className="rounded-md bg-red-100 px-3 py-1 text-md transition hover:bg-red-200"
        >
          See&nbsp;Details
        </Link>
      </div>

      {/* short description */}
      {description && (
        <p className="mt-1 line-clamp-2 text-xs text-gray-600">{description}</p>
      )}
    </article>
  );
}

Product.propTypes = {
  product: PropTypes.shape({
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    _id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    name: PropTypes.string,
    title: PropTypes.string,
    description: PropTypes.string,
    price: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    imageUrl: PropTypes.string,
    thumbnail: PropTypes.string,
    images: PropTypes.arrayOf(PropTypes.string),
  }).isRequired,
};
