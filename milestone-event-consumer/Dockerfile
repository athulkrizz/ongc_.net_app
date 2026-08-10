# Use Node.js 18 Alpine for smaller image size
FROM node:18-alpine AS builder

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./
COPY tsconfig.json ./

# Install all dependencies (including dev dependencies for build)
RUN npm ci

# Copy application source
COPY src/ ./src/
COPY scripts/ ./scripts/

# Build TypeScript
RUN npm run build

# Production stage
FROM node:18-alpine

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install production dependencies only
RUN npm ci --only=production

# Copy built application from builder
COPY --from=builder /app/dist ./dist

# Create logs directory
RUN mkdir -p logs

# Expose health check port
EXPOSE 3000

# Set environment to production
ENV NODE_ENV=production

# Run the application
CMD ["node", "dist/index.js"]
