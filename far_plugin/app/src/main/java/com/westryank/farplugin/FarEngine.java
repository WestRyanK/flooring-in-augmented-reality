package com.westryank.farplugin;

import android.content.Context;
import android.graphics.Bitmap;
import android.util.Log;

import org.tensorflow.lite.DataType;
import org.tensorflow.lite.support.image.TensorImage;

import java.nio.ByteBuffer;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class FarEngine {
    private static final String TAG = "Far:Engine";

    private Context _context;
    private OutputResultTypeEnum _outputResultType;

    private SegmentationModel _segmentationModel;
    private SegmentationTuner _segmentationTuner;
    private LightingExtractor _lightingExtractor;

    private TensorImage _inputTensor;
    private Bitmap _inputBitmap;

    private ByteBuffer _segmentationBuffer;
    private ByteBuffer _lightingBuffer;
    private Bitmap _segmentationBitmap;

    private Lock _segmentationModelLock = new ReentrantLock();

    public FarEngine(Context inContext, SegmentationModelTypeEnum inSegmentationModelType, OutputResultTypeEnum inOutputResultType) {
        _context = inContext;
        InitSegmentationModel(inSegmentationModelType);
        InitSegmentationTuner(inOutputResultType);
        InitLightingExtractor();
    }

    public byte[] SegmentInputImage() {
        _segmentationModelLock.lock();
        Bitmap initialSegmentationBitmap = null;
        try {
            initialSegmentationBitmap = _segmentationModel.Infer(_inputTensor);
        } catch (Exception e) {
            Log.e(TAG, e.toString());
            return new byte[0];
        } finally {
            _segmentationModelLock.unlock();
        }

//        if (_segmentationBitmap != null) {
//            _segmentationBitmap.recycle();
//        }

        if (_outputResultType == OutputResultTypeEnum.NoFineTuning) {
            _segmentationBitmap = _segmentationTuner.ConvertMaskBitmap(initialSegmentationBitmap);
        } else {
            _segmentationBitmap = _segmentationTuner.Finetune(_inputBitmap, initialSegmentationBitmap);
        }

        _segmentationBuffer = CopyBitmapToBuffer(_segmentationBitmap, _segmentationBuffer);
        initialSegmentationBitmap.recycle();
        return _segmentationBuffer.array();
    }

    public byte[] ExtractInputImageLighting() {
        Bitmap lightingBitmap = _lightingExtractor.ExtractLighting(_inputBitmap, _segmentationBitmap);

        _lightingBuffer = CopyBitmapToBuffer(lightingBitmap, _lightingBuffer);
//        lightingBitmap.recycle();
        return _lightingBuffer.array();
    }

    private ByteBuffer CopyBitmapToBuffer(Bitmap inBitmap, ByteBuffer inBuffer) {
        if (inBitmap == null) {
            Log.e(TAG, "CopyBitmapToBuffer: inBitmap is null");
            return inBuffer;
        }
        int size = inBitmap.getRowBytes() * inBitmap.getHeight();
        if (inBuffer == null || size != inBuffer.capacity()) {
            inBuffer = ByteBuffer.allocate(size);
            Log.d(TAG, "CopyBitmapToBuffer: buffer size: " + Integer.toString(size));
        }

        inBuffer.position(0);
        inBitmap.copyPixelsToBuffer(inBuffer);

        return inBuffer;
    }

    public void InitSegmentationTuner(OutputResultTypeEnum inOutputResultType) {
        _outputResultType = inOutputResultType;
        try {
            if (_segmentationTuner != null) {
                _segmentationTuner.close();
            }
            Log.v(TAG, "Initializing SegmentationTuner");
            _segmentationTuner = new SegmentationTuner(inOutputResultType);
            Log.v(TAG, "SegmentationTuner Initialized");
        } catch (Exception e) {
            Log.e(TAG, "Error initializing SegmentationTuner", e);
        }
    }

    public void InitSegmentationModel(SegmentationModelTypeEnum inSegmentationModelType) {
        try {
            _segmentationModelLock.lock();
            if (_segmentationModel != null) {
                _segmentationModel.close();
            }
            Log.v(TAG, "Initializing SegmentationModel");
            _segmentationModel = new SegmentationModel(_context, inSegmentationModelType);
            Log.v(TAG, "SegmentationModel Initialized");
        } catch (Exception e) {
            Log.e(TAG, "Error initializing SegmentationModel", e);
        } finally {
            _segmentationModelLock.unlock();
        }
    }

    public void InitLightingExtractor() {
        try {
            if (_lightingExtractor != null) {
                _lightingExtractor.close();
            }
            Log.v(TAG, "Initializing ShadowExtractor");
            _lightingExtractor = new LightingExtractor();
        } catch (Exception e) {
            Log.v(TAG, "ShadowExtractor Initialized", e);
        }
    }


    public void SetInputImage(int[] inInputImage, int inWidth, int inHeight) {
        if (inInputImage == null) {
            Log.e(TAG, "SetInputImage: inInputImage is null");
            return;
        }

        if (_inputTensor == null) {
            _inputTensor = new TensorImage(DataType.UINT8);
        }

        int[] shape = {inHeight, inWidth, 3};
        _inputTensor.load(inInputImage, shape);

        if (_inputBitmap != null) {
            _inputBitmap.recycle();
        }
        _inputBitmap = _inputTensor.getBitmap();
    }

//    private void PreviewBitmap(String inName, int[] inInputImage) {
//        StringBuilder sb = new StringBuilder();
//        for (int i = 0; i < 200; i++) {
//            int r = inInputImage[i * 3 + 0];
//            int g = inInputImage[i * 3 + 1];
//            int b = inInputImage[i * 3 + 2];
//            sb.append(r);
//            sb.append(" ");
//            sb.append(g);
//            sb.append(" ");
//            sb.append(b);
//            sb.append("  ");
//        }
//        Log.v(TAG, inName + " " + sb.toString());
//    }
//
//    private void PreviewBitmap(String inName, Bitmap inInputImage) {
//        StringBuilder sb = new StringBuilder();
//        for (int i = 0; i < 200; i++) {
//            int color = inInputImage.getPixel(i, 0);
//            sb.append(Color.red(color));
//            sb.append(" ");
//            sb.append(Color.green(color));
//            sb.append(" ");
//            sb.append(Color.blue(color));
//            sb.append("  ");
//        }
//        Log.v(TAG, inName + " " + sb.toString());
//    }
}
