import React from "react";
import axios from "axios";

const baseURL = "http://localhost:5267/api";

const axiosClient = axios.create({
  baseURL: "http://localhost:5267/api",
  withCredentials: true, // send cookies (refresh token)
  headers: {
    "Content-Type": "application/json",
  }
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  })

  failedQueue = [];
}

axiosClient.interceptors.request.use(
  config => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error)
);

axiosClient.interceptors.response.use(
  response => response,
  error => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // queue requests while refreshing
        return new Promise(function(resolve, reject) {
          failedQueue.push({ resolve, reject });
        }).then(token => {
          originalRequest.headers.Authorization = 'Bearer ' + token;
          return axiosClient(originalRequest);
        }).catch(err => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      return new Promise((resolve, reject) => {
        axiosClient.post("/auth/refresh")
          .then(({ data }) => {
            localStorage.setItem("token", data.token);
            axiosClient.defaults.headers.common.Authorization = 'Bearer ' + data.token;
            originalRequest.headers.Authorization = 'Bearer ' + data.token;
            processQueue(null, data.token);
            resolve(axiosClient(originalRequest));
          })
          .catch(err => {
            processQueue(err, null);
            reject(err);
            // optionally trigger logout here (clear token, user state)
          })
          .finally(() => {
            isRefreshing = false;
          });
      });
    }

    return Promise.reject(error);
  }
);

export default axiosClient;
