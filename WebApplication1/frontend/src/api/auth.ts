const API_BASE_URL = 'http://localhost:5065/api';

export interface RegisterPayload {
  fullName: string;
  email: string;
  mobile: string;
  username: string;
  password: string;
  confirmPassword: string;
  profileImage?: File;
  captcha: string;
}

export interface RegisterResponse {
  success: boolean;
  message: string;
  profileImageName?: string;
  errors?: string[];
}

export interface OtpResponse {
  message: string;
}

export async function registerUser(payload: RegisterPayload): Promise<RegisterResponse> {
  const formData = new FormData();
  formData.append('fullName', payload.fullName);
  formData.append('email', payload.email);
  formData.append('mobile', payload.mobile);
  formData.append('username', payload.username);
  formData.append('password', payload.password);
  formData.append('confirmPassword', payload.confirmPassword);
  formData.append('captcha', payload.captcha);

  if (payload.profileImage) {
    formData.append('profileImage', payload.profileImage);
  }

  const response = await fetch(`${API_BASE_URL}/auth/register`, {
    method: 'POST',
    body: formData,
  });

  return response.json();
}

export async function sendOtp(identifier: string): Promise<OtpResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/send-otp`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier }),
  });

  return response.json();
}

export async function verifyOtp(identifier: string, otp: string): Promise<OtpResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/verify-otp`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier, otp }),
  });

  return response.json();
}
