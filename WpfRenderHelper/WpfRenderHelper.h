// WpfRenderHelper.h

#pragma once

//#include <msclr\lock.h>
//#include <msclr\marshal_cppstd.h>
//using namespace msclr::interop;

using namespace System;
using namespace System::Windows::Media;
using namespace System::Windows::Media::Imaging;

namespace WpfRenderHelper {
    

	public ref class WpfRenderHelper
	{
    public:
        event System::Action<array<Byte>^>^ NewWriteableImage;
        
        void Worker() 
        { 
            System::Console::WriteLine("working!");

            array<Byte>^ bgra = gcnew array<Byte>(640*480*4);

            for (int i=0;i<640*480*4;i++) {
				bgra[i] = 0x99; // Just a transparent gray square
            }

			while(true) {
                NewWriteableImage(bgra);
                Threading::Thread::Sleep(33); // 30fps-ish
            }
        }
	};
}
