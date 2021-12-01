package com.westryank.farplugin;

import android.content.Context;
import android.graphics.Bitmap;
import android.util.Log;

import org.tensorflow.lite.support.image.TensorImage;
import org.tensorflow.lite.task.vision.segmenter.ImageSegmenter;
import org.tensorflow.lite.task.vision.segmenter.Segmentation;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.channels.Channels;
import java.util.List;

public class SegmentationModel implements AutoCloseable{
    private static final String TAG = "Far:SegmentationModel";

    private ImageSegmenter _imageSegmenter;
    private SegmentationModelTypeEnum _segmentationModelType;

    public SegmentationModel(Context inContext, SegmentationModelTypeEnum inModel) {
        _segmentationModelType = inModel;
        try {
            String modelFilename = inModel.GetFilename();
            Log.v(TAG, "Loading Segmentation model " + modelFilename);
            ByteBuffer buffer = LoadFileToBuffer(inContext, modelFilename);
            _imageSegmenter = ImageSegmenter.createFromBuffer(buffer);
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }
    }

    private ByteBuffer LoadFileToBuffer(Context inContext, String inFilename) throws IOException {
        ByteBuffer buffer = null;
        InputStream stream = inContext.getAssets().open(inFilename);
        buffer = ByteBuffer.allocateDirect(stream.available());
        Channels.newChannel(stream).read(buffer);
        return buffer;
    }

    public Bitmap Infer(TensorImage inInputTensor) {
        if (inInputTensor == null) {
            Log.e(TAG, "Infer: inInputTensor is null");
            return null;
        }

        try {
            List<Segmentation> results = _imageSegmenter.segment(inInputTensor);
            Bitmap maskBitmap = results.get(0).getMasks().get(0).getBitmap();

            Bitmap scaledBitmap = Bitmap.createScaledBitmap(maskBitmap, inInputTensor.getWidth(), inInputTensor.getHeight(), true);
            Bitmap argb8888Bitmap = scaledBitmap.copy(Bitmap.Config.ARGB_8888, false);
            scaledBitmap.recycle();
            maskBitmap.recycle();
            return argb8888Bitmap;

        } catch (Exception e) {
            Log.e(TAG, "Infer: Error occurred", e);
            return null;
        }
    }

    @Override
    public void close() throws Exception {
        if (_imageSegmenter != null && !_imageSegmenter.isClosed()) {
            _imageSegmenter.close();
        }
    }
}
