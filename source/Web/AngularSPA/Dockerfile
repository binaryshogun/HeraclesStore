# !!!
# Image should be builded from source directory
# !!!

# NodeJS image
FROM node:latest as build
# Working directory
WORKDIR /src
# Copy project files
COPY ./Web/AngularSPA/ ./Web/AngularSPA/
# Copy package information
COPY ./Web/AngularSPA/package.json ./Web/AngularSPA/
# Use SPA directory
WORKDIR /src/Web/AngularSPA
# Install packages
RUN npm install
# Build application
RUN npm run build --prod

# Hosting over NGINX 
FROM nginx:alpine

# Use source directory
WORKDIR /src
# Copy NGINX config file
COPY ./Web/AngularSPA/nginx.conf /etc/nginx/nginx.conf
# Copy project files and set them as NGINX server content
COPY --from=build src/Web/AngularSPA/dist/angular-spa /usr/share/nginx/html

EXPOSE 80