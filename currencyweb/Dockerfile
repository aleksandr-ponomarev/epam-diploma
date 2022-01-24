FROM node:16.13.1-stretch as build

ARG API_ADDRESS

WORKDIR /usr/app

# Copy packege dependency file and install npm packages. This step to cache "npm install" stage.
COPY package.json package-lock.json ./
RUN npm i

# Copy rest of source files and build main app.
ENV REACT_APP_API_ADDRESS=$API_ADDRESS
COPY . .
RUN npm run build


FROM nginx:1.20.2-alpine

# Copy built files in prev stage to nginx html folder. Doing this we would get final image with size 25MB only.
COPY --from=build /usr/app/build /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]