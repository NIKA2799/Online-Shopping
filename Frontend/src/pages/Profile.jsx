import { useAuth } from "@/context/AuthContext";

const Profile = () => {
  const { user, logout } = useAuth();

  return (
    <div className="p-4">
      <h2 className="text-xl font-bold">Welcome, {user?.email}</h2>
      <p>Role: {user?.role || "user"}</p>
      <button onClick={logout} className="bg-red-500 text-white px-4 py-2 mt-4">
        Logout
      </button>
    </div>
  );
};

export default Profile;
