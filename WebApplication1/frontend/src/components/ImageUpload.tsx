import React, { useEffect } from 'react';

interface ImageUploadProps {
  onImageSelect: (file: File | null) => void;
  preview: string;
  setPreview: (url: string) => void;
}

export default function ImageUpload({ onImageSelect, preview, setPreview }: ImageUploadProps) {
  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];

    if (!file) return;

    if (file.size > 5 * 1024 * 1024) {
      alert('Image must be less than 5 MB');
      return;
    }

    if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type)) {
      alert('Only JPG, PNG and WebP images are allowed');
      return;
    }

    onImageSelect(file);
    setPreview(URL.createObjectURL(file));
  };

  useEffect(() => {
    return () => {
      if (preview) URL.revokeObjectURL(preview);
    };
  }, [preview]);

  return (
    <div className="image-upload-section">
      <label className="form-label">Profile Photo</label>
      <div className="profile-upload">
        {preview ? (
          <img src={preview} alt="Profile preview" className="profile-preview" />
        ) : (
          <div className="upload-placeholder">
            <span className="camera-icon">📷</span>
            <p>Upload Profile Photo</p>
            <small>JPG, PNG or WebP • Max 5 MB</small>
          </div>
        )}
        <input
          type="file"
          accept="image/jpeg,image/png,image/webp"
          onChange={handleImageChange}
          className="file-input"
        />
      </div>
      {preview && (
        <p className="image-optimized-note">
          Image optimized automatically
          <br />
          <small>Your image will be optimized for faster upload while maintaining high visual quality.</small>
        </p>
      )}
    </div>
  );
}
