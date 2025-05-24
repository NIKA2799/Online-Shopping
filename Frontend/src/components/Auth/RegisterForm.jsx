import { useState } from "react";
import { registerUser } from "@/api/auth";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";

const RegisterForm = () => {
  const [form, setForm] = useState({ email: "", password: "" });
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    const data = await registerUser(form);
    login(data.token);
    navigate("/profile");
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 p-4 max-w-md mx-auto">
      <input
        type="email"
        placeholder="Email"
        value={form.email}
        onChange={(e) => setForm({ ...form, email: e.target.value })}
        className="border p-2 w-full"
      />
      <input
        type="password"
        placeholder="Password"
        value={form.password}
        onChange={(e) => setForm({ ...form, password: e.target.value })}
        className="border p-2 w-full"
      />
      <button
        type="submit"
        className="bg-blue-600 text-white py-2 px-4 rounded"
      >
        Register
      </button>
    </form>
  );
};

export default RegisterForm;
