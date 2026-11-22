export class ApiError extends Error {
  statusCode: number;

  constructor(statusCode: number, message?: string) {
    super(message);
    this.name = "ApiError";
    this.statusCode = statusCode;
  }
}

export class AutoLoginError extends Error {
  constructor(message = "Не удалось выполнить вход после регистрации") {
    super(message);
    this.name = "AutoLoginError";
  }
}
