#ifndef NODE_FILEOUTPUT_H_
#define NODE_FILEOUTPUT_H_

namespace netease 
{
	class FileOutputStream: public OutputStream
	{
	public:
		FileOutputStream(FILE* stream): stream_(stream)
		{
		}

		virtual int GetChunkSize()
		{
    return 65536; // big chunks == faster
}

virtual void EndOfStream()
{
}

virtual WriteResult WriteAsciiChunk(char* data, int size)
{
	const size_t len = static_cast<size_t>(size);
	size_t off = 0;

	while (off < len && !feof(stream_) && !ferror(stream_))
		off += fwrite(data + off, 1, len - off, stream_);

	return off == len ? kContinue : kAbort;
}

private:
	FILE* stream_;
};

}

#endif // NODE_FILEOUTPUT_H_
