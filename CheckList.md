# ✅ HTTPS Setup Checklist for ccflock.duckdns.org

This guide documents how to switch from HTTP to HTTPS using Let's Encrypt and Dockerized NGINX.

---

## 🔧 Prerequisites

- Certbot already installed and certificate already issued on the **server**
- Domain: `ccflock.duckdns.org`
- Certificates located at:
  - `/etc/letsencrypt/live/ccflock.duckdns.org/fullchain.pem`
  - `/etc/letsencrypt/live/ccflock.duckdns.org/privkey.pem`
- Docker Compose is used to manage services

---

## 🧱 Step 1: Update `docker-compose.yml`

In the `nginx` service block:

```yaml
ports:
  - "80:80"
  - "443:443"

volumes:
  - ./nginx/conf:/etc/nginx/conf.d:ro
  - ./frontend:/usr/share/nginx/html:ro
  - /etc/letsencrypt:/etc/letsencrypt:ro
  - /var/lib/letsencrypt:/var/lib/letsencrypt:ro
```

Code to be updated to:

```config
server {
    listen 443 ssl;
    server_name ccflock.duckdns.org;

    ssl_certificate /etc/letsencrypt/live/ccflock.duckdns.org/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/ccflock.duckdns.org/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    return 301 https://$host$request_uri;

    location = /login.html {
        root /usr/share/nginx/html;
    }

    location = /hub.html {
        root /usr/share/nginx/html;
    }

    location = /register.html {
        root /usr/share/nginx/html;
    }

    location /api/login {
        proxy_pass http://login-service:80/Login/login;
    }

    location /api/register {
        proxy_pass http://login-service:80/Login/register;
    }

    location /api/online-users {
        proxy_pass http://hub-service:80/OnlineUsers/onlineusers;
    }

    location /api/heartbeat {
        proxy_pass http://hub-service:80/Heartbeat/sendheartbeat;
        proxy_set_header Authorization $http_authorization;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }

    location / {
        root /usr/share/nginx/html;
        index login.html;
        try_files $uri $uri/ =404;
    }
}

```
