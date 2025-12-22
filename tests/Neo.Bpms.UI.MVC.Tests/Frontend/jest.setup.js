// Jest setup file
// This file runs before each test file

// Setup jsdom environment
require('jest-environment-jsdom');

// Mock global objects if needed
global.alert = jest.fn();

