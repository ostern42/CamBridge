// src/CamBridge.Core/Interfaces/IFileProcessor.cs
// Version: 0.7.0
// Description: KISS UPDATE - This interface is no longer needed and can be deleted!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

// KISS STEP 1.2: This entire file can be deleted!
// FileProcessor now follows the ExifToolReader/DicomConverter pattern
// with direct dependencies instead of interfaces.

/*
 * This interface was removed as part of THE GREAT SIMPLIFICATION (Sprint 7)
 * 
 * What changed:
 * - FileProcessor.cs no longer implements IFileProcessor
 * - ProcessingQueue.cs uses FileProcessor directly
 * - ServiceCollectionExtensions.cs registers FileProcessor directly
 * 
 * Benefits:
 * - Less abstraction layers
 * - Easier to understand
 * - No unnecessary interfaces for single implementations
 * - Following the KISS principle!
 */

// DELETE THIS FILE or keep it commented for documentation purposes
