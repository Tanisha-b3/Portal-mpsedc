import { useState } from 'react';
import { registerUser, sendOtp, type RegisterPayload } from '../api/auth';
import ImageUpload from '../components/ImageUpload';
import './RegistrationPage.css';

interface FormErrors {
  [key: string]: string;
}

export default function RegistrationPage() {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    mobile: '',
    username: '',
    password: '',
    confirmPassword: '',
    captcha: '',
  });

  const [image, setImage] = useState<File | null>(null);
  const [preview, setPreview] = useState('');
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [apiError, setApiError] = useState('');
  const [otpSent, setOtpSent] = useState(false);
  const [otpVerified, setOtpVerified] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: '' }));
    }
  };

  const validate = (): boolean => {
    const newErrors: FormErrors = {};

    if (!formData.fullName || formData.fullName.length < 2) {
      newErrors.fullName = 'Full name must be at least 2 characters';
    }

    if (!formData.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Valid email is required';
    }

    if (!formData.mobile || !/^[6-9]\d{9}$/.test(formData.mobile)) {
      newErrors.mobile = 'Invalid Indian mobile number';
    }

    if (!formData.username || formData.username.length < 4 || !/^[a-zA-Z0-9_]+$/.test(formData.username)) {
      newErrors.username = 'Username must be 4+ chars (letters, numbers, underscore)';
    }

    if (!formData.password || formData.password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    } else {
      if (!/[A-Z]/.test(formData.password)) newErrors.password = 'Password must contain uppercase';
      else if (!/[a-z]/.test(formData.password)) newErrors.password = 'Password must contain lowercase';
      else if (!/[0-9]/.test(formData.password)) newErrors.password = 'Password must contain a digit';
      else if (!/[!@#$%^&*]/.test(formData.password)) newErrors.password = 'Password must contain !@#$%^&*';
    }

    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }

    if (!formData.captcha) {
      newErrors.captcha = 'CAPTCHA is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSendOtp = async () => {
    if (!formData.email) {
      setErrors((prev) => ({ ...prev, email: 'Enter email first' }));
      return;
    }
    try {
      await sendOtp(formData.email);
      setOtpSent(true);
    } catch {
      setApiError('Failed to send OTP');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setApiError('');

    if (!validate()) return;

    setLoading(true);

    try {
      const payload: RegisterPayload = {
        fullName: formData.fullName,
        email: formData.email,
        mobile: formData.mobile,
        username: formData.username,
        password: formData.password,
        confirmPassword: formData.confirmPassword,
        captcha: formData.captcha,
      };

      if (image) {
        payload.profileImage = image;
      }

      const response = await registerUser(payload);

      if (response.success) {
        setSuccess(true);
      } else {
        setApiError(response.message || 'Registration failed');
        if (response.errors) {
          const apiErrors: FormErrors = {};
          response.errors.forEach((err) => {
            if (err.includes('Email')) apiErrors.email = err;
            else if (err.includes('Username')) apiErrors.username = err;
            else if (err.includes('Password')) apiErrors.password = err;
          });
          setErrors((prev) => ({ ...prev, ...apiErrors }));
        }
      }
    } catch {
      setApiError('Network error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <div className="registration-container">
        <header className="header">
          <div className="header-content">
            <div className="logo">
              <span className="logo-icon">🏛️</span>
              <span className="logo-text">MPSEDC</span>
            </div>
            <div className="header-right">
              <button className="theme-toggle">☀</button>
              <button className="lang-toggle">हिन्दी</button>
            </div>
          </div>
        </header>

        <main className="main-content">
          <div className="success-card">
            <div className="success-icon">✅</div>
            <h2>Registration Successful!</h2>
            <p>Your account has been created. Please check your email for OTP verification.</p>
            <p className="otp-hint">Mock OTP: <strong>123456</strong></p>
            <button className="btn-primary" onClick={() => window.location.reload()}>
              Go to Login
            </button>
          </div>
        </main>

        <footer className="footer">
          <p>Government of Madhya Pradesh | MPSEDC</p>
        </footer>
      </div>
    );
  }

  return (
    <div className="registration-container">
      <header className="header">
        <div className="header-content">
          <div className="logo">
            <span className="logo-icon">🏛️</span>
            <span className="logo-text">MPSEDC</span>
          </div>
          <div className="header-right">
            <button className="theme-toggle">☀</button>
            <button className="lang-toggle">हिन्दी</button>
          </div>
        </div>
      </header>

      <main className="main-content">
        <div className="registration-card">
          <div className="card-header">
            <h1>Create Your MPSEDC Account</h1>
            <p>Register to access MPSEDC online services</p>
          </div>

          <form onSubmit={handleSubmit} className="registration-form">
            {apiError && <div className="api-error">{apiError}</div>}

            <div className="form-section">
              <h2 className="section-title">Personal Information</h2>

              <div className="form-row">
                <div className="form-group">
                  <label className="form-label" htmlFor="fullName">Full Name</label>
                  <input
                    type="text"
                    id="fullName"
                    name="fullName"
                    value={formData.fullName}
                    onChange={handleChange}
                    className={`form-input ${errors.fullName ? 'error' : ''}`}
                    placeholder="Enter your full name"
                  />
                  {errors.fullName && <span className="error-text">{errors.fullName}</span>}
                </div>

                <div className="form-group">
                  <label className="form-label" htmlFor="mobile">Mobile Number</label>
                  <input
                    type="tel"
                    id="mobile"
                    name="mobile"
                    value={formData.mobile}
                    onChange={handleChange}
                    className={`form-input ${errors.mobile ? 'error' : ''}`}
                    placeholder="10-digit mobile number"
                    maxLength={10}
                  />
                  {errors.mobile && <span className="error-text">{errors.mobile}</span>}
                </div>
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label className="form-label" htmlFor="email">Email Address</label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    className={`form-input ${errors.email ? 'error' : ''}`}
                    placeholder="your.email@example.com"
                  />
                  {errors.email && <span className="error-text">{errors.email}</span>}
                  {otpSent && <span className="otp-hint">OTP sent! Mock: 123456</span>}
                </div>

                <div className="form-group">
                  <label className="form-label" htmlFor="username">Username</label>
                  <input
                    type="text"
                    id="username"
                    name="username"
                    value={formData.username}
                    onChange={handleChange}
                    className={`form-input ${errors.username ? 'error' : ''}`}
                    placeholder="Choose a username"
                  />
                  {errors.username && <span className="error-text">{errors.username}</span>}
                </div>
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label className="form-label" htmlFor="password">Password</label>
                  <input
                    type="password"
                    id="password"
                    name="password"
                    value={formData.password}
                    onChange={handleChange}
                    className={`form-input ${errors.password ? 'error' : ''}`}
                    placeholder="Min 8 chars with uppercase, lowercase, digit, special char"
                  />
                  {errors.password && <span className="error-text">{errors.password}</span>}
                </div>

                <div className="form-group">
                  <label className="form-label" htmlFor="confirmPassword">Confirm Password</label>
                  <input
                    type="password"
                    id="confirmPassword"
                    name="confirmPassword"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    className={`form-input ${errors.confirmPassword ? 'error' : ''}`}
                    placeholder="Re-enter your password"
                  />
                  {errors.confirmPassword && <span className="error-text">{errors.confirmPassword}</span>}
                </div>
              </div>

              <ImageUpload
                onImageSelect={setImage}
                preview={preview}
                setPreview={setPreview}
              />

              <div className="form-group captcha-group">
                <label className="form-label">CAPTCHA Verification</label>
                <div className="captcha-box">
                  <div className="captcha-image">
                    <span className="captcha-text">MOCK CAPTCHA</span>
                    <small>For demo purposes</small>
                  </div>
                  <input
                    type="text"
                    name="captcha"
                    value={formData.captcha}
                    onChange={handleChange}
                    className={`form-input captcha-input ${errors.captcha ? 'error' : ''}`}
                    placeholder="Enter CAPTCHA"
                  />
                </div>
                {errors.captcha && <span className="error-text">{errors.captcha}</span>}
              </div>

              <div className="form-group checkbox-group">
                <label className="checkbox-label">
                  <input type="checkbox" required />
                  <span>I agree to the <a href="#" className="link">Terms & Conditions</a></span>
                </label>
              </div>

              <button type="submit" className="btn-primary" disabled={loading}>
                {loading ? (
                  <span className="loading-spinner">
                    <span className="spinner"></span>
                    Creating Account...
                  </span>
                ) : (
                  'Create Account'
                )}
              </button>
            </div>
          </form>

          <div className="card-footer">
            <p>Already registered? <a href="#" className="link">Login</a></p>
          </div>
        </div>
      </main>

      <footer className="footer">
        <p>Government of Madhya Pradesh | MPSEDC</p>
      </footer>
    </div>
  );
}
