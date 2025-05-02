import PropTypes from "prop-types";

export default function Product({ product }) {
  return (
    <article className="border border-gray-200 rounded-lg p-4 text-center">
      {product.imageUrl && (
        <img
          src={product.imageUrl}
          alt={product.name}
          className="w-full h-40 object-cover rounded mb-2"
        />
      )}

      <h2 className="text-base font-medium truncate">{product.name}</h2>

      {product.price != null && (
        <p className="text-sm font-semibold mt-1">
          ${Number(product.price).toFixed(2)}
        </p>
      )}

      {product.description && (
        <p className="text-xs text-gray-600 mt-1 truncate">
          {product.description}
        </p>
      )}
    </article>
  );
}

Product.propTypes = {
  product: PropTypes.shape({
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    name: PropTypes.string.isRequired,
    description: PropTypes.string,
    price: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    imageUrl: PropTypes.string,
  }).isRequired,
};
