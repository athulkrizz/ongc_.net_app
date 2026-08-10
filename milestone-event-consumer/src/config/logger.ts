import winston from 'winston';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const logLevel = process.env.LOG_LEVEL || 'info';
const nodeEnv = process.env.NODE_ENV || 'development';

// Define log format
const logFormat = winston.format.combine(
  winston.format.timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
  winston.format.errors({ stack: true }),
  winston.format.splat(),
  winston.format.json()
);

// Console format for development
const consoleFormat = winston.format.combine(
  winston.format.colorize(),
  winston.format.timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
  winston.format.printf(({ timestamp, level, message, ...metadata }) => {
    let msg = `${timestamp} [${level}]: ${message}`;
    if (Object.keys(metadata).length > 0) {
      msg += ` ${JSON.stringify(metadata)}`;
    }
    return msg;
  })
);

// Create transports array
const transports: winston.transport[] = [
  // Console transport
  new winston.transports.Console({
    format: nodeEnv === 'development' ? consoleFormat : logFormat
  })
];

// Add file transports for production
if (nodeEnv === 'production') {
  const logsDir = path.join(path.dirname(__dirname), '..', 'logs');
  
  transports.push(
    // Error log
    new winston.transports.File({
      filename: path.join(logsDir, 'error.log'),
      level: 'error',
      format: logFormat,
      maxsize: 10485760, // 10MB
      maxFiles: 5
    }),
    // Combined log
    new winston.transports.File({
      filename: path.join(logsDir, 'app.log'),
      format: logFormat,
      maxsize: 10485760, // 10MB
      maxFiles: 5
    })
  );
}

// Create logger instance
const logger = winston.createLogger({
  level: logLevel,
  format: logFormat,
  transports,
  exitOnError: false
});

export default logger;
