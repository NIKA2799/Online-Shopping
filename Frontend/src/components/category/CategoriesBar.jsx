import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import PropTypes from "prop-types";

/* -------------------------------------------------------------
 * Fetch & normalise categories – works with either
 *  • ["smartphones", "laptops", ...]  (string[])
 *  • [{ slug, name, url }, ...]       (object[])
 * ------------------------------------------------------------- */
const fetchCategories = async () => {
  const { data } = await axios.get("https://dummyjson.com/products/categories");

  /* normalise everything to { slug, name } */
  const toObj = (c) =>
    typeof c === "string"
      ? { slug: c, name: c }
      : { slug: c.slug, name: c.name ?? c.slug };

  return (Array.isArray(data) ? data : []).map(toObj);
};

export default function CategoriesBar({ selected, onSelect }) {
  const [expanded, setExpanded] = useState(false);

  const {
    data: categories = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["categories", "all"],
    queryFn: fetchCategories,
    staleTime: 60 * 60 * 1000,
    cacheTime: 60 * 60 * 1000,
    refetchOnWindowFocus: false,
  });

  if (isLoading) return <div className="p-4">Loading categories…</div>;
  if (isError)
    return <div className="p-4 text-red-600">Couldn’t load categories.</div>;

  /* prepend the “all” pseudo‑categorys */
  const all = [{ slug: "all", name: "All products" }, ...categories];

  return (
    <section className="relative p-4">
      <div
        className={`flex gap-3 ${
          expanded
            ? "flex-wrap overflow-visible"
            : "flex-nowrap overflow-x-auto"
        }`}
      >
        {all.map((cat) => (
          <button
            key={cat.slug}
            onClick={() => onSelect(cat.slug)}
            className={`whitespace-nowrap rounded-full border px-4 py-1 text-sm transition
              ${
                selected === cat.slug
                  ? "border-indigo-600 bg-indigo-50 font-semibold text-indigo-700"
                  : "border-gray-300 hover:bg-gray-100"
              }`}
          >
            {cat.name}
          </button>
        ))}
      </div>

      {!expanded && (
        <button
          onClick={() => setExpanded(true)}
          className="absolute bottom-2 right-6 rounded bg-gray-50 px-2 py-0.5 text-xs font-medium text-gray-600 shadow-sm hover:bg-gray-100"
        >
          See&nbsp;more
        </button>
      )}
    </section>
  );
}

CategoriesBar.propTypes = {
  selected: PropTypes.string.isRequired,
  onSelect: PropTypes.func.isRequired,
};
