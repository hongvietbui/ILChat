import { LoginForm } from "@/components/auth/login-form";

export default function LoginPage() {
  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="w-full max-w-md p-6 rounded-2xl shadow-xl border">
        <h1 className="text-2xl font-semibold text-center mb-4">Login</h1>
        <LoginForm />
      </div>
    </div>
  );
}
