using Microsoft.SPOT.Hardware;

namespace Hardware {

    public static class SPI1 {

        private static Microsoft.SPOT.Hardware.SPI _spi = null;

        /* start with a dummy config */
        private static SPI.Configuration _config = new SPI.Configuration(Cpu.Pin.GPIO_NONE, false, 0, 0, false, true, 12000, SPI.SPI_module.SPI1);

        public static SPI Get(SPI.Configuration config)
        {
            Config = config;
            return _spi;
        }


        private static SPI Spi {
            get {
                if(_spi == null)
                    _spi = new Microsoft.SPOT.Hardware.SPI(_config);
                return _spi;
            }
        }
        private static SPI.Configuration Config {
            set {
                if(_config == value) {
                    /* nothing to do */
                } else {
                    Spi.Config = value;
                }
            }
        }

        //private static Cpu.Pin _cs = Cpu.Pin.GPIO_NONE;

        //private static SPI.Configuration _spiConfig = new SPI.Configuration(Cpu.Pin.GPIO_NONE, false, 0, 0, false, true, 12000, SPI.SPI_module.SPI1);

        //public static SPI.Configuration Config {
        //    get {
        //        return _spiConfig;
        //    }
        //}

        //public static Cpu.Pin ChipSel {
        //    set {
        //        if(_cs != ChipSel) {
        //            _cs = ChipSel;

        //            _spiConfig = null;
        //            Debug.GC(true);
        //            _spiConfig = new SPI.Configuration(_cs, false, 0, 0, false, true, 12000, SPI.SPI_module.SPI1);
        //        }
        //    }
        //    get {
        //        return _cs;
        //    }
        //}
    }
}
