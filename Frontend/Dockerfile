#base image
FROM node:latest
WORKDIR /app
COPY . .
COPY ["src/config.docker.json", "src/config.json"]

# ==== BUILD =====
RUN npm ci
RUN npm run build

# ==== RUN =======
ENV NODE_ENV production
EXPOSE 443
EXPOSE 80

CMD [ "npx", "serve", "build", "--listen", "443", "--ssl-cert", "/https/server.crt", "--ssl-key", "/https/server.key"]