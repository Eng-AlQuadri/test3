import { HttpInterceptorFn } from '@angular/common/http';

export const languageInterceptor: HttpInterceptorFn = (req, next) => {

  const language = localStorage.getItem("schoolNet.language") ?? "en";

  req = req.clone({
    setHeaders: {
      "custom-language": language
    }
  })
  return next(req);
};
