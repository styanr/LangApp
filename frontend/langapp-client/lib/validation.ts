import * as EmailValidator from 'email-validator';

/**
 * Username validation constants and functions
 */
export const USERNAME_VALIDATION = {
  MIN_LENGTH: 4,
  MAX_LENGTH: 20,
  // Regex to validate username:
  // - Cannot start with _ or .
  // - Cannot end with _ or .
  // - Cannot have consecutive _ or .
  // - Can only contain alphanumeric, underscore, and period
  REGEX: /^(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$/,
};

export const validateUsername = (username: string) => {
  const isTooShort = username.length < USERNAME_VALIDATION.MIN_LENGTH;
  const isTooLong = username.length > USERNAME_VALIDATION.MAX_LENGTH;
  const hasValidFormat = USERNAME_VALIDATION.REGEX.test(username);

  return {
    isValid: !isTooShort && !isTooLong && hasValidFormat,
    isTooShort,
    isTooLong,
    hasValidFormat: hasValidFormat,
  };
};

/**
 * Email validation
 */
export const validateEmail = (email: string) => {
  return EmailValidator.validate(email);
};

/**
 * Name validation
 */
export const validateName = (name: string) => {
  return name.trim().length > 0;
};

/**
 * Password validation constants and functions
 */
export const PASSWORD_VALIDATION = {
  MIN_LENGTH: 8,
  REQUIRES_UPPERCASE: true,
  REQUIRES_LOWERCASE: true,
  REQUIRES_NUMBER: true,
  REQUIRES_SYMBOL: true,
  SYMBOL_REGEX: /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/,
};

export const validatePassword = (password: string) => {
  const hasMinLength = password.length >= PASSWORD_VALIDATION.MIN_LENGTH;
  const hasUppercase = /[A-Z]/.test(password);
  const hasLowercase = /[a-z]/.test(password);
  const hasNumber = /[0-9]/.test(password);
  const hasSymbol = PASSWORD_VALIDATION.SYMBOL_REGEX.test(password);

  return {
    isValid:
      hasMinLength &&
      (!PASSWORD_VALIDATION.REQUIRES_UPPERCASE || hasUppercase) &&
      (!PASSWORD_VALIDATION.REQUIRES_LOWERCASE || hasLowercase) &&
      (!PASSWORD_VALIDATION.REQUIRES_NUMBER || hasNumber) &&
      (!PASSWORD_VALIDATION.REQUIRES_SYMBOL || hasSymbol),
    hasMinLength,
    hasUppercase,
    hasLowercase,
    hasNumber,
    hasSymbol,
  };
};

/**
 * Password match validation
 */
export const passwordsMatch = (password: string, confirmPassword: string) => {
  return password === confirmPassword && password !== '';
};

/**
 * Form fields touched state handler
 * Creates an object to track which fields have been interacted with
 */
export const createTouchedState = <T extends Record<string, any>>(fields: T) => {
  const touchedState: Record<keyof T, boolean> = {} as Record<keyof T, boolean>;

  // Initialize all fields as untouched
  for (const key in fields) {
    touchedState[key] = false;
  }

  return touchedState;
};
